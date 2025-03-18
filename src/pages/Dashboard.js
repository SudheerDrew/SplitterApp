import React, { useState, useEffect } from 'react';
import { getGroups, createGroup } from '../services/groupService';
import GroupCard from '../components/GroupCard';
import Sidebar from '../components/Sidebar';

const Dashboard = () => {
  const [groups, setGroups] = useState([]); // Stores all groups
  const [loading, setLoading] = useState(true); // Manages loading state
  const [error, setError] = useState(''); // Error message
  const [showCreateGroup, setShowCreateGroup] = useState(false); // Modal visibility
  const [newGroupName, setNewGroupName] = useState(''); // New group name input
  const [createError, setCreateError] = useState(''); // Create group error message
  const [createSuccess, setCreateSuccess] = useState(''); // Success message
  const [creating, setCreating] = useState(false); // To handle button loading state

  // Fetch groups from backend when component mounts
  useEffect(() => {
    const fetchGroups = async () => {
      try {
        const token = localStorage.getItem('jwtToken');
        if (!token) {
          setError('You must be logged in to view groups.');
          setLoading(false);
          return;
        }

        const response = await getGroups(token); // Call to service
        console.log('Fetched Groups:', response); // Debugging log
        setGroups(response); // Update state with fetched groups
      } catch (err) {
        console.error('Error fetching groups:', err.message); // Log error
        setError(err.message || 'Failed to load groups. Please try again.');
      } finally {
        setLoading(false);
      }
    };

    fetchGroups();
  }, []);

  // Handle Create Group functionality
  const handleCreateGroup = async () => {
    setCreating(true); // Start loading
    try {
      console.log('Create Group button clicked!'); // Debugging log

      const token = localStorage.getItem('jwtToken');
      if (!token) {
        setCreateError('Authorization token is missing. Please log in again.');
        return;
      }

      if (!newGroupName.trim()) {
        setCreateError('Group name cannot be empty.');
        return;
      }

      const newGroup = { groupName: newGroupName }; // Group creation object
      console.log('Creating group with data:', newGroup); // Debugging log

      const createdGroup = await createGroup(newGroup, token); // Call to service
      console.log('Group created successfully:', createdGroup); // Debugging log

      setGroups((prevGroups) => [...prevGroups, createdGroup]); // Update group list
      setShowCreateGroup(false); // Close modal
      setNewGroupName(''); // Reset input
      setCreateError(''); // Clear any error messages
      setCreateSuccess('Group created successfully!'); // Show success message

      // Clear success message after 3 seconds
      setTimeout(() => setCreateSuccess(''), 3000);
    } catch (err) {
      console.error('Error creating group:', err.response || err.message); // Log error
      setCreateError(err.response?.data?.message || 'Failed to create group. Please try again.');
    } finally {
      setCreating(false); // Stop loading
    }
  };

  return (
    <div className="d-flex">
      <Sidebar />
      <div className="container-fluid mt-3">
        <h2>Dashboard</h2>

        {/* Create Group Modal Trigger */}
        <button
          className="btn btn-primary mb-3"
          onClick={() => {
            setShowCreateGroup(true);
            console.log('Create Group modal opened'); // Debugging
          }}
        >
          Create Group
        </button>

        {/* Loading, Error, or Success States */}
        {loading && <p>Loading...</p>}
        {error && <div className="alert alert-danger">{error}</div>}
        {createSuccess && <div className="alert alert-success">{createSuccess}</div>}

        {/* Display Groups */}
        {!loading && !error && (
          <div className="row">
            {groups.length > 0 ? (
              groups.map((group) => (
                <div key={group.groupID} className="col-md-4">
                  <GroupCard group={group} />
                </div>
              ))
            ) : (
              <p>No groups found. Create one to get started!</p>
            )}
          </div>
        )}

        {/* Create Group Modal */}
        {showCreateGroup && (
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
            }}
          >
            <div
              className="modal-content"
              style={{
                background: '#fff',
                padding: '20px',
                borderRadius: '8px',
                width: '400px',
                textAlign: 'center',
              }}
            >
              <h4>Create New Group</h4>
              {createError && <div className="alert alert-danger">{createError}</div>}

              {/* Input for Group Name */}
              <input
                type="text"
                className="form-control"
                placeholder="Enter group name (e.g., Friends Expenses, Family Expenses)"
                value={newGroupName}
                onChange={(e) => setNewGroupName(e.target.value)}
              />

              {/* Create Group Button */}
              <button
                className="btn btn-success mt-3"
                onClick={handleCreateGroup}
                disabled={creating} // Disable button during API call
              >
                {creating ? 'Creating...' : 'Create Group'}
              </button>

              {/* Cancel Button */}
              <button
                className="btn btn-secondary mt-3"
                onClick={() => {
                  setShowCreateGroup(false); // Close modal
                  setCreateError(''); // Clear error
                  setNewGroupName(''); // Reset input
                }}
              >
                Cancel
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Dashboard;
