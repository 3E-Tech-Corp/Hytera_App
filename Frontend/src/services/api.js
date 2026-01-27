import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || '/api'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Add auth token to requests
apiClient.interceptors.request.use((config) => {
  const storedUser = localStorage.getItem('hytera_user')
  if (storedUser) {
    try {
      const user = JSON.parse(storedUser)
      if (user.accessToken) {
        config.headers.Authorization = `Bearer ${user.accessToken}`
      }
    } catch (e) {
      // Invalid stored user
    }
  }
  return config
})

// Handle auth errors
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('hytera_user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export const api = {
  // Auth
  login: async (credentials) => {
    const response = await apiClient.post('/User/FastLogin', credentials)
    return response.data
  },

  resetPassword: async (email) => {
    const response = await apiClient.get(`/User/ResetPassword/${encodeURIComponent(email)}`)
    return response.data
  },

  updatePassword: async (data) => {
    const response = await apiClient.post('/User/ResetPassword', data)
    return response.data
  },

  // Inventory
  checkInventory: async (itemCode) => {
    const response = await apiClient.get(`/Api/Inventory/${encodeURIComponent(itemCode)}`)
    return response.data
  },

  searchInventory: async (itemCode) => {
    const response = await apiClient.post('/Api/Inventory', { itemCode })
    return response.data
  },

  // NLU Inventory
  nluInventory: async (text) => {
    const response = await apiClient.post('/api/nlu/inventory', { text })
    return response.data
  },

  // Game Scores
  uploadScore: async (scoreData) => {
    const response = await apiClient.post('/Game/UploadScore', scoreData)
    return response.data
  },

  checkScore: async (scoreQuery) => {
    const response = await apiClient.post('/Game/CheckScore', scoreQuery)
    return response.data
  },

  // App
  checkVersion: async (os) => {
    const response = await apiClient.get(`/App/CheckNewVersion/${encodeURIComponent(os)}`)
    return response.data
  },

  getLanguages: async (code = 'all') => {
    const response = await apiClient.get(`/App/Language/${encodeURIComponent(code)}`)
    return response.data
  },

  // Voice Sets
  getVoiceSets: async (userId) => {
    const response = await apiClient.get(`/User/Voiceset/${encodeURIComponent(userId)}`)
    return response.data
  },

  // Assets
  getImageUrl: (imageId, width = 800, height = 600) => {
    return `${API_BASE_URL}/Asset/Image/${imageId}/${width}/${height}`
  },

  getFileUrl: (fileId) => {
    return `${API_BASE_URL}/Asset/File/${fileId}`
  }
}

export default api
