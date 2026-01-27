import { useState } from 'react'

function Orders() {
  const [activeTab, setActiveTab] = useState('orders')

  const tabs = [
    { id: 'orders', label: 'Orders' },
    { id: 'rma', label: 'RMA' }
  ]

  return (
    <div className="p-4">
      <h2 className="text-xl font-semibold text-gray-900 mb-4">Orders & RMA</h2>

      {/* Tabs */}
      <div className="flex border-b border-gray-200 mb-4">
        {tabs.map((tab) => (
          <button
            key={tab.id}
            onClick={() => setActiveTab(tab.id)}
            className={`flex-1 py-3 text-sm font-medium border-b-2 transition-colors ${
              activeTab === tab.id
                ? 'border-primary-600 text-primary-600'
                : 'border-transparent text-gray-500 hover:text-gray-700'
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {/* Content */}
      {activeTab === 'orders' ? (
        <OrdersList />
      ) : (
        <RMAList />
      )}
    </div>
  )
}

function OrdersList() {
  // Placeholder data - in a real app, this would come from an API
  const orders = []

  if (orders.length === 0) {
    return (
      <div className="card text-center py-12">
        <svg className="w-12 h-12 mx-auto text-gray-400 mb-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
        </svg>
        <p className="text-gray-500">No orders found</p>
        <p className="text-sm text-gray-400 mt-1">Your orders will appear here</p>
      </div>
    )
  }

  return (
    <div className="space-y-4">
      {orders.map((order) => (
        <div key={order.id} className="card">
          <div className="flex justify-between items-start">
            <div>
              <h3 className="font-medium text-gray-900">Order #{order.id}</h3>
              <p className="text-sm text-gray-500">{order.date}</p>
            </div>
            <span className={`px-2 py-1 rounded-full text-xs font-medium ${
              order.status === 'delivered' ? 'bg-green-100 text-green-700' :
              order.status === 'shipped' ? 'bg-blue-100 text-blue-700' :
              'bg-yellow-100 text-yellow-700'
            }`}>
              {order.status}
            </span>
          </div>
          <p className="mt-2 text-sm text-gray-600">{order.items} items</p>
          <p className="text-sm font-medium text-gray-900">${order.total}</p>
        </div>
      ))}
    </div>
  )
}

function RMAList() {
  // Placeholder data
  const rmas = []

  if (rmas.length === 0) {
    return (
      <div className="card text-center py-12">
        <svg className="w-12 h-12 mx-auto text-gray-400 mb-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
        </svg>
        <p className="text-gray-500">No RMA requests found</p>
        <p className="text-sm text-gray-400 mt-1">Your return requests will appear here</p>
      </div>
    )
  }

  return (
    <div className="space-y-4">
      {rmas.map((rma) => (
        <div key={rma.id} className="card">
          <div className="flex justify-between items-start">
            <div>
              <h3 className="font-medium text-gray-900">RMA #{rma.id}</h3>
              <p className="text-sm text-gray-500">{rma.date}</p>
            </div>
            <span className={`px-2 py-1 rounded-full text-xs font-medium ${
              rma.status === 'approved' ? 'bg-green-100 text-green-700' :
              rma.status === 'pending' ? 'bg-yellow-100 text-yellow-700' :
              'bg-gray-100 text-gray-700'
            }`}>
              {rma.status}
            </span>
          </div>
          <p className="mt-2 text-sm text-gray-600">{rma.reason}</p>
        </div>
      ))}
    </div>
  )
}

export default Orders
