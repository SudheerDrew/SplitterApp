import React, { useState } from 'react';
import { addExpense } from '../services/expenseService';

const decodeJWT = (token) => {
  const base64Url = token.split('.')[1];
  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  const jsonPayload = decodeURIComponent(
    atob(base64)
      .split('')
      .map((c) => `%${`00${c.charCodeAt(0).toString(16)}`.slice(-2)}`)
      .join('')
  );
  return JSON.parse(jsonPayload);
};

const ExpenseForm = ({ groupId, onSuccess }) => {
  const [formData, setFormData] = useState({
    description: '',
    amount: '',
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    setError('');
    setSuccess('');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    try {
      const token = localStorage.getItem('jwtToken');
      const decodedToken = decodeJWT(token);
      const userId = decodedToken.UserID; // Extract UserID from token

      const expenseData = {
        description: formData.description,
        amount: parseFloat(formData.amount),
        groupId: parseInt(groupId, 10),
        userId: userId
      };

      await addExpense(expenseData, token);
      setSuccess('Expense added successfully!');
      onSuccess(); // Refresh group details
      setFormData({ description: '', amount: '' });
    } catch (err) {
      setError('Failed to add expense. Please try again.');
    }
  };

  return (
    <form onSubmit={handleSubmit} className="mt-4">
      <h5>Add an Expense</h5>
      {error && <div className="alert alert-danger">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}
      <div className="mb-3">
        <label htmlFor="description" className="form-label">Description</label>
        <input
          type="text"
          className="form-control"
          id="description"
          name="description"
          value={formData.description}
          onChange={handleChange}
          required
        />
      </div>
      <div className="mb-3">
        <label htmlFor="amount" className="form-label">Amount</label>
        <input
          type="number"
          className="form-control"
          id="amount"
          name="amount"
          value={formData.amount}
          onChange={handleChange}
          required
        />
      </div>
      <button type="submit" className="btn btn-primary w-100">Add Expense</button>
    </form>
  );
};

export default ExpenseForm;
