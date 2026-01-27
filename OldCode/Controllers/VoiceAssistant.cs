using OpenAI;
using OpenAI.Chat;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text.Json;

public class VoiceInventoryAgent
{
    private readonly ChatClient _chatClient;
    private readonly SpeechRecognitionEngine _speechEngine;
    private readonly SpeechSynthesizer _speechSynthesizer;
    private readonly List<ChatMessage> _conversationHistory;

    public VoiceInventoryAgent(string apiKey)
    {
        var openAIClient = new OpenAIClient(apiKey);
        _chatClient = openAIClient.GetChatClient("gpt-4");
        _speechEngine = new SpeechRecognitionEngine();
        _speechSynthesizer = new SpeechSynthesizer();
        _conversationHistory = new List<ChatMessage>();

        InitializeConversation();
        InitializeSpeechRecognition();
    }

    private void InitializeConversation()
    {
        _conversationHistory.Add(ChatMessage.CreateSystemMessage(
            "You are a helpful inventory management assistant. " +
            "Use the check_inventory function to get real-time stock information. " +
            "Provide clear, concise responses about inventory levels."
        ));
    }

    private void InitializeSpeechRecognition()
    {
        _speechEngine.SetInputToDefaultAudioDevice();

        var choices = new Choices();
        choices.Add(new string[] {
            "check inventory", "stock level", "how many",
            "do we have", "inventory status", "stock check"
        });

        var grammar = new Grammar(new GrammarBuilder(choices));
        _speechEngine.LoadGrammar(grammar);
        _speechEngine.SpeechRecognized += OnSpeechRecognized;
    }

    private async void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
    {
        if (e.Result.Confidence > 0.7)
        {
            Console.WriteLine($"Recognized: {e.Result.Text}");
            await ProcessVoiceCommand(e.Result.Text);
        }
    }

    public async Task ProcessVoiceCommand(string voiceInput)
    {
        try
        {
            // Add user message to conversation
            _conversationHistory.Add(ChatMessage.CreateUserMessage(voiceInput));

            // Define available functions
            var chatTools = new List<ChatTool>
            {
                ChatTool.CreateFunctionTool(
                    functionName: "check_inventory",
                    functionDescription: "Check current stock levels for a product",
                    functionParameters: BinaryData.FromString("""
                    {
                        "type": "object",
                        "properties": {
                            "product_name": {
                                "type": "string",
                                "description": "Name of the product to check"
                            },
                            "location": {
                                "type": "string",
                                "description": "Optional: specific warehouse or location"
                            }
                        },
                        "required": ["product_name"]
                    }
                    """)
                )
            };

            // Make chat completion request
            var chatOptions = new ChatCompletionOptions
            {
                Tools = chatTools
            };

            var response = await _chatClient.CompleteChatAsync(_conversationHistory, chatOptions);
            var responseMessage = response.Value.Content[0];

            // Handle function calls
            if (response.Value.FinishReason == ChatFinishReason.ToolCalls)
            {
                await HandleFunctionCalls(response.Value);
            }
            else
            {
                // Regular response
                var assistantResponse = responseMessage.Text;
                _conversationHistory.Add(ChatMessage.CreateAssistantMessage(assistantResponse));

                Console.WriteLine($"Assistant: {assistantResponse}");
                await SpeakResponse(assistantResponse);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            await SpeakResponse("Sorry, I encountered an error processing your request.");
        }
    }

    private async Task HandleFunctionCalls(ChatCompletion chatCompletion)
    {
        // Add assistant message with tool calls
        _conversationHistory.Add(ChatMessage.CreateAssistantMessage(chatCompletion.ToolCalls));

        foreach (var toolCall in chatCompletion.ToolCalls)
        {
            if (toolCall.FunctionName == "check_inventory")
            {
                var arguments = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                    toolCall.FunctionArguments);

                var productName = arguments["product_name"].GetString();
                var location = arguments.ContainsKey("location") ?
                    arguments["location"].GetString() : null;

                var inventoryResult = await CheckInventory(productName, location);

                // Add function result to conversation
                _conversationHistory.Add(ChatMessage.CreateToolMessage(toolCall.Id, inventoryResult));
            }
        }

        // Get final response after function calls
        var finalResponse = await _chatClient.CompleteChatAsync(_conversationHistory);
        var finalMessage = finalResponse.Value.Content[0].Text;

        _conversationHistory.Add(ChatMessage.CreateAssistantMessage(finalMessage));

        Console.WriteLine($"Assistant: {finalMessage}");
        await SpeakResponse(finalMessage);
    }

    private async Task<string> CheckInventory(string productName, string location = null)
    {
        // Your inventory database logic here
        await Task.Delay(100); // Simulate database call

        var mockInventory = new Dictionary<string, (int quantity, string location)>
        {
            {"iphone case", (25, "Warehouse A")},
            {"wireless headphones", (15, "Warehouse B")},
            {"samsung phone", (8, "Store Front")},
            {"laptop charger", (42, "Warehouse A")}
        };

        var key = mockInventory.Keys.FirstOrDefault(k =>
            k.Contains(productName.ToLower()) ||
            productName.ToLower().Contains(k));

        if (key != null)
        {
            var (quantity, itemLocation) = mockInventory[key];
            return $"Found {quantity} units of {key} in {itemLocation}";
        }

        return $"Product '{productName}' not found in inventory";
    }

    private async Task SpeakResponse(string text)
    {
        await Task.Run(() =>
        {
            _speechSynthesizer.Rate = 0;
            _speechSynthesizer.Volume = 100;
            _speechSynthesizer.Speak(text);
        });
    }

    public void StartListening()
    {
        Console.WriteLine("Voice inventory agent is listening...");
        _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
    }

    public void StopListening()
    {
        _speechEngine.RecognizeAsyncStop();
    }

    public void Dispose()
    {
        _speechEngine?.Dispose();
        _speechSynthesizer?.Dispose();
    }
}