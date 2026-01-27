import { createContext, useContext, useState, useEffect } from 'react'
import { api } from '../services/api'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    // Check for stored user on mount
    const storedUser = localStorage.getItem('hytera_user')
    if (storedUser) {
      try {
        setUser(JSON.parse(storedUser))
      } catch (e) {
        localStorage.removeItem('hytera_user')
      }
    }
    setLoading(false)
  }, [])

  const login = async (email, password, appId = 'HyteraApp') => {
    try {
      const response = await api.login({ appId, email, password })
      if (response.status >= 0) {
        const userData = {
          userId: response.userId,
          firstName: response.firstName,
          lastName: response.lastName,
          email: email,
          accessToken: response.accessToken,
          userRole: response.userRole,
          userRoleName: response.userRoleName,
          bpCode: response.bpCode,
          bpName: response.bpName
        }
        setUser(userData)
        localStorage.setItem('hytera_user', JSON.stringify(userData))
        return { success: true, user: userData }
      } else {
        return { success: false, message: response.message || 'Login failed' }
      }
    } catch (error) {
      return { success: false, message: error.message || 'Network error' }
    }
  }

  const logout = () => {
    setUser(null)
    localStorage.removeItem('hytera_user')
  }

  const value = {
    user,
    loading,
    login,
    logout,
    isAuthenticated: !!user
  }

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}
