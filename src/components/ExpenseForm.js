import React, { useState } from 'react';
import { addExpense } from '../services/expenseService';

const ExpenseForm = ({ groupId, onSuccess }) => {
  const [formData, setFormData] = useState({
    description: '',
    amount: '',
    addedBy: '',
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    try {
      const token = localStorage.getItem('jwtToken'); // Retrieve token
      await addExpense({ groupId, ...formData }, token);
      setSuccess('Expense added successfully!');
      onSuccess(); // Callback to refresh the group details
      setFormData({ description: '', amount: '', addedBy: '' }); // Reset form
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
      <div className="mb-3">
        <label htmlFor="addedBy" className="form-label">Added By</label>
        <input
          type="text"
          className="form-control"
          id="addedBy"
          name="addedBy"
          value={formData.addedBy}
          onChange={handleChange}
          required
        />
      </div>
      <button type="submit" className="btn btn-primary w-100">Add Expense</button>
    </form>
  );
};

export default ExpenseForm;
