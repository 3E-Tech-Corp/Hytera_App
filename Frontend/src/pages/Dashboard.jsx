import { useAuth } from '../context/AuthContext'
import { Link } from 'react-router-dom'

function Dashboard() {
  const { user } = useAuth()

  const quickActions = [
    {
      title: 'Check Inventory',
      description: 'Search for product availability',
      icon: InventoryIcon,
      link: '/inventory',
      color: 'bg-blue-500'
    },
    {
      title: 'View Orders',
      description: 'Track your orders and RMAs',
      icon: OrdersIcon,
      link: '/orders',
      color: 'bg-green-500'
    },
    {
      title: 'Settings',
      description: 'Manage your account',
      icon: SettingsIcon,
      link: '/settings',
      color: 'bg-purple-500'
    }
  ]

  return (
    <div className="p-4">
      {/* Welcome Section */}
      <div className="card mb-6">
        <h2 className="text-xl font-semibold text-gray-900">
          Welcome, {user?.firstName || 'User'}!
        </h2>
        <p className="text-gray-600 mt-1">
          {user?.bpName && `${user.bpName}`}
        </p>
      </div>

      {/* Quick Actions */}
      <h3 className="text-lg font-medium text-gray-900 mb-3">Quick Actions</h3>
      <div className="grid gap-4">
        {quickActions.map((action) => (
          <Link
            key={action.title}
            to={action.link}
            className="card flex items-center gap-4 hover:shadow-md transition-shadow"
          >
            <div className={`${action.color} p-3 rounded-lg`}>
              <action.icon className="w-6 h-6 text-white" />
            </div>
            <div>
              <h4 className="font-medium text-gray-900">{action.title}</h4>
              <p className="text-sm text-gray-500">{action.description}</p>
            </div>
            <svg className="w-5 h-5 text-gray-400 ml-auto" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
            </svg>
          </Link>
        ))}
      </div>

      {/* Recent Activity */}
      <h3 className="text-lg font-medium text-gray-900 mt-6 mb-3">Recent Activity</h3>
      <div className="card">
        <p className="text-gray-500 text-center py-4">No recent activity</p>
      </div>
    </div>
  )
}

// Icons
function InventoryIcon({ className }) {
  return (
    <svg className={className} fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
    </svg>
  )
}

function OrdersIcon({ className }) {
  return (
    <svg className={className} fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
    </svg>
  )
}

function SettingsIcon({ className }) {
  return (
    <svg className={className} fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
    </svg>
  )
}

export default Dashboard
