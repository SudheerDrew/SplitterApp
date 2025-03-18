import React, { useEffect, useState, useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { getGroupDetails } from '../services/groupService';
import GroupCard from '../components/GroupCard';
import ExpenseForm from '../components/ExpenseForm';
import PaymentForm from '../components/PaymentForm';
import BalanceSummary from '../components/BalanceSummary';
import AddMembersForm from '../components/AddMembersForm';

const GroupDetails = () => {
  const { groupId } = useParams(); // Retrieves groupId from URL params
  const [group, setGroup] = useState(null); // Stores the group details
  const [error, setError] = useState(''); // Error message
  const [loading, setLoading] = useState(true); // Manages loading state
  const [showAddMembersModal, setShowAddMembersModal] = useState(false); // Manages Add Members modal visibility

  // Fetch Group Details from the API
  const fetchGroupDetails = useCallback(async () => {
    try {
      setLoading(true); // Show loading spinner
      console.log('groupId from URL:', groupId); // Debugging groupId

      const token = localStorage.getItem('jwtToken');
      if (!token) {
        console.error('Token is missing.');
        throw new Error('Authorization token is missing.');
      }

      // Make API call to fetch group details
      const response = await getGroupDetails(groupId, token);
      console.log('Fetched Group Details:', response); // Debugging response
      setGroup(response); // Set group details in state
      setError(''); // Clear any previous errors
    } catch (err) {
      console.error('Error fetching group details:', err.response || err.message); // Log error
      if (err.response) {
        console.error('Backend Error Details:', err.response.data); // Detailed backend errors
      }
      setError(err.response?.data?.message || 'Failed to load group details. Please try again.');
      setGroup(null); // Reset group if fetch fails
    } finally {
      setLoading(false); // Stop loading spinner
    }
  }, [groupId]);

  // Fetch details when the component mounts or groupId changes
  useEffect(() => {
    fetchGroupDetails();
  }, [fetchGroupDetails]);

  if (loading) {
    return <div className="container mt-5">Loading group details...</div>;
  }

  return (
    <div className="container mt-5">
      <h2>Group Details</h2>

      {/* Display error message if present */}
      {error && <div className="alert alert-danger">{error}</div>}

      {/* Display group details if available */}
      {group ? (
        <>
          <h3>{group.groupName}</h3>

          {/* Group Summary */}
          <GroupCard group={group} />

          {/* Balance Summary */}
          <h4>Balance Summary</h4>
          {group.balanceSummary && group.balanceSummary.length > 0 ? (
            <BalanceSummary balanceSummary={group.balanceSummary} />
          ) : (
            <p className="text-muted">No balance information available.</p>
          )}

          {/* Manage Members Section */}
          <div className="mt-4">
            <h5>Manage Members</h5>
            <button
              className="btn btn-secondary mb-3"
              onClick={() => setShowAddMembersModal(true)}
            >
              Add Members
            </button>

            {/* Add Members Modal */}
            {showAddMembersModal && (
              <div
                className="modal"
                style={{
                  position: 'fixed',
                  top: 0,
                  left: 0,
                  width: '100%',
                  height: '100%',
                  background: 'rgba(0, 0, 0, 0.5)',
                  display: 'flex',
                  justifyContent: 'center',
                  alignItems: 'center',
                  zIndex: 1000,
                }}
              >
                <div
                  className="modal-content"
                  style={{
                    background: '#fff',
                    padding: '20px',
                    borderRadius: '8px',
                    width: '400px',
                    maxWidth: '90%',
                    boxShadow: '0 4px 10px rgba(0, 0, 0, 0.25)',
                    textAlign: 'center',
                  }}
                >
                  <AddMembersForm groupId={groupId} onSuccess={fetchGroupDetails} />
                  <button
                    className="btn btn-secondary mt-3"
                    onClick={() => setShowAddMembersModal(false)}
                  >
                    Close
                  </button>
                </div>
              </div>
            )}
          </div>

          {/* Expenses Section */}
          <h4>Expenses</h4>
          {group.expenses && group.expenses.length > 0 ? (
            <ul>
              {group.expenses.map((expense) => (
                <li key={expense.expenseID}>
                  <strong>{expense.description}</strong>: ₹{expense.amount} (Paid by {expense.paidBy || 'Unknown'})
                </li>
              ))}
            </ul>
          ) : (
            <p className="text-muted">No expenses found for this group.</p>
          )}

          {/* Add New Expense */}
          <ExpenseForm groupId={groupId} onSuccess={fetchGroupDetails} />

          {/* Add Payment */}
          <PaymentForm groupId={groupId} members={group.members} onSuccess={fetchGroupDetails} />
        </>
      ) : (
        <div className="alert alert-warning">Group details are not available.</div>
      )}
    </div>
  );
};

export default GroupDetails;
