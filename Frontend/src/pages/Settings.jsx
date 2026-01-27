import { useAuth } from '../context/AuthContext'

function Settings() {
  const { user } = useAuth()

  return (
    <div className="p-4">
      <h2 className="text-xl font-semibold text-gray-900 mb-4">Settings</h2>

      {/* Profile Section */}
      <div className="card mb-4">
        <h3 className="font-medium text-gray-900 mb-3">Profile</h3>
        <div className="space-y-3">
          <div className="flex justify-between">
            <span className="text-gray-500">Name</span>
            <span className="text-gray-900">{user?.firstName} {user?.lastName}</span>
          </div>
          <div className="flex justify-between">
            <span className="text-gray-500">Email</span>
            <span className="text-gray-900">{user?.email}</span>
          </div>
          <div className="flex justify-between">
            <span className="text-gray-500">Role</span>
            <span className="text-gray-900">{user?.userRoleName || user?.userRole || 'User'}</span>
          </div>
          {user?.bpName && (
            <div className="flex justify-between">
              <span className="text-gray-500">Business Partner</span>
              <span className="text-gray-900">{user.bpName}</span>
            </div>
          )}
        </div>
      </div>

      {/* App Info */}
      <div className="card mb-4">
        <h3 className="font-medium text-gray-900 mb-3">App Information</h3>
        <div className="space-y-3">
          <div className="flex justify-between">
            <span className="text-gray-500">Version</span>
            <span className="text-gray-900">1.0.0</span>
          </div>
          <div className="flex justify-between">
            <span className="text-gray-500">Build</span>
            <span className="text-gray-900">2024.01</span>
          </div>
        </div>
      </div>

      {/* Actions */}
      <div className="space-y-3">
        <button className="w-full card flex items-center justify-between hover:shadow-md transition-shadow">
          <span className="text-gray-900">Change Password</span>
          <svg className="w-5 h-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
          </svg>
        </button>

        <button className="w-full card flex items-center justify-between hover:shadow-md transition-shadow">
          <span className="text-gray-900">Notifications</span>
          <svg className="w-5 h-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
          </svg>
        </button>

        <button className="w-full card flex items-center justify-between hover:shadow-md transition-shadow">
          <span className="text-gray-900">Help & Support</span>
          <svg className="w-5 h-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
          </svg>
        </button>
      </div>

      {/* PWA Install Prompt */}
      <div className="mt-6 card bg-primary-50 border-primary-200">
        <h3 className="font-medium text-primary-900 mb-2">Install App</h3>
        <p className="text-sm text-primary-700 mb-3">
          Install Hytera App on your device for a better experience
        </p>
        <button className="btn btn-primary text-sm">
          Install Now
        </button>
      </div>
    </div>
  )
}

export default Settings
