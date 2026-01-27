import { useState } from 'react'
import { api } from '../services/api'

function Inventory() {
  const [searchQuery, setSearchQuery] = useState('')
  const [results, setResults] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSearch = async (e) => {
    e.preventDefault()
    if (!searchQuery.trim()) return

    setLoading(true)
    setError('')
    setResults(null)

    try {
      const response = await api.searchInventory(searchQuery)
      if (response.status >= 0) {
        setResults(response.itemsFound || [])
      } else {
        setError(response.message || 'Search failed')
      }
    } catch (err) {
      setError('Network error. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="p-4">
      <h2 className="text-xl font-semibold text-gray-900 mb-4">Inventory Search</h2>

      {/* Search Form */}
      <form onSubmit={handleSearch} className="mb-6">
        <div className="flex gap-2">
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Enter item code..."
            className="input flex-1"
          />
          <button
            type="submit"
            disabled={loading}
            className="btn btn-primary px-6"
          >
            {loading ? (
              <svg className="animate-spin h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
              </svg>
            ) : (
              'Search'
            )}
          </button>
        </div>
      </form>

      {/* Error */}
      {error && (
        <div className="p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm mb-4">
          {error}
        </div>
      )}

      {/* Results */}
      {results && (
        <div>
          {results.length === 0 ? (
            <div className="card text-center py-8">
              <p className="text-gray-500">No items found</p>
            </div>
          ) : (
            <div className="space-y-4">
              {results.map((item) => (
                <div key={item.itemId} className="card">
                  <div className="flex justify-between items-start">
                    <div>
                      <h3 className="font-medium text-gray-900">{item.itemName}</h3>
                      <p className="text-sm text-gray-500">{item.itemCode}</p>
                    </div>
                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                      item.instock > 0 ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'
                    }`}>
                      {item.instock > 0 ? 'In Stock' : 'Out of Stock'}
                    </span>
                  </div>

                  <div className="mt-3 grid grid-cols-2 gap-4 text-sm">
                    <div>
                      <span className="text-gray-500">In Stock:</span>
                      <span className="ml-2 font-medium">{item.instock}</span>
                    </div>
                    <div>
                      <span className="text-gray-500">On Order:</span>
                      <span className="ml-2 font-medium">{item.onOrder}</span>
                    </div>
                    <div>
                      <span className="text-gray-500">MSRP:</span>
                      <span className="ml-2 font-medium">${item.msrp?.toFixed(2)}</span>
                    </div>
                    <div>
                      <span className="text-gray-500">Dealer Price:</span>
                      <span className="ml-2 font-medium">${item.dealerPrice?.toFixed(2)}</span>
                    </div>
                  </div>

                  {item.itemType && (
                    <p className="mt-2 text-xs text-gray-400">Type: {item.itemType}</p>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {/* Empty State */}
      {!results && !loading && (
        <div className="card text-center py-12">
          <svg className="w-12 h-12 mx-auto text-gray-400 mb-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          <p className="text-gray-500">Search for inventory items by code</p>
        </div>
      )}
    </div>
  )
}

export default Inventory
