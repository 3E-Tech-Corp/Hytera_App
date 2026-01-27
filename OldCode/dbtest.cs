using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using YYTools;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using static FunTimePIE.Controllers.CheckNewVersionController;

namespace FunTimePIE.Controllers
{


    [Route("Pie/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private string constr = @"Data Source=.\;Initial Catalog=FunTime;User ID=sa;Password=arthur1028;";
        DBTool mydbt = new DBTool(@"Data Source=.\;Initial Catalog=FunTime;User ID=sa;Password=arthur1028;");

        private static List<TodoItem> _todos = new List<TodoItem>
        {
            new TodoItem { Id = 1, Title = "Todo 1", Completed = false },
            new TodoItem { Id = 2, Title = "Todo 2", Completed = true }
        };

        [HttpGet]
        public ActionResult<IEnumerable<TodoItem>> Get()
        {
            return Ok(_todos);
        }

        [HttpGet("{id}")]
        public ActionResult<Player> Get(int id)
        {
            Player P = new Player();
            mydbt.AddParam("User_ID", id);
            DataSet DS = mydbt.RunCMD("csp_User_Get_By_ID");
            if (DS.Tables[0].Rows.Count > 0)
            {
                P.Id = id;
                P.FName = DS.Tables[0].Rows[0]["Fname"].ToString();
                P.LName = DS.Tables[0].Rows[0]["Lname"].ToString();
                P.EMail = DS.Tables[0].Rows[0]["EMail"].ToString();
                return Ok(P);
            }

            return NotFound();

        }

        [HttpPost]
        public ActionResult<TodoItem> Post(TodoItem todo)
        {
            todo.Id = _todos.Count + 1;
            _todos.Add(todo);
            return CreatedAtAction(nameof(Get), new { id = todo.Id }, todo);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, TodoItem todo)
        {

            var index = _todos.FindIndex(t => t.Id == id);
            if (index == -1)
            {
                return NotFound();
            }
            _todos[index] = todo;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var index = _todos.FindIndex(t => t.Id == id);
            if (index == -1)
            {
                return NotFound();
            }
            _todos.RemoveAt(index);
            return NoContent();
        }
    }

    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
    }

    public class Player
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string EMail { get; set; }
    }
}
