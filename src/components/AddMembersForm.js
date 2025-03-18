import React, { useState } from 'react';
import { addMembers } from '../services/groupService'; // Import the addMembers method

const AddMembersForm = ({ groupId, onSuccess }) => {
  const [userIDs, setUserIDs] = useState(''); // Comma-separated user IDs
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    try {
      const token = localStorage.getItem('jwtToken');
      if (!token) {
        throw new Error('Authorization token is missing. Please log in again.');
      }

      // Convert input into an array of integers
      const userIDArray = userIDs.split(',').map((id) => parseInt(id.trim(), 10));
      console.log('Submitting User IDs:', userIDArray); // Debug log

      // Use the addMembers service to make the API call
      await addMembers(groupId, userIDArray, token);

      setSuccess('Members added successfully!');
      onSuccess(); // Refresh group details
      setUserIDs(''); // Reset form field
    } catch (err) {
      console.error('Error adding members:', err.response || err); // Log the error
      console.log('Full Error Response:', err.response); // Log additional details
      const errorMessage =
        err.response?.data?.message || 'Failed to add members. Please try again.';
      setError(errorMessage); // Display the error message
    }
  };

  return (
    <form onSubmit={handleSubmit} className="mt-4">
      <h5>Add Members</h5>
      {error && <div className="alert alert-danger">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}
      <div className="mb-3">
        <label htmlFor="userIDs" className="form-label">User IDs (comma-separated)</label>
        <input
          type="text"
          className="form-control"
          id="userIDs"
          name="userIDs"
          value={userIDs}
          onChange={(e) => setUserIDs(e.target.value)}
          placeholder="Enter User IDs, e.g., 2, 3"
          required
        />
      </div>
      <button type="submit" className="btn btn-primary w-100">Add Members</button>
    </form>
  );
};

export default AddMembersForm;
