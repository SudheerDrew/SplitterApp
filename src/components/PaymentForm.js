import React, { useState } from 'react';
import { addPayment } from '../services/paymentService';

const PaymentForm = ({ groupId, members, onSuccess }) => {
  const [formData, setFormData] = useState({
    payerId: '',
    payeeId: '',
    amount: '',
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
      await addPayment({ groupId, ...formData }, token); // Call payment service
      setSuccess('Payment recorded successfully!');
      onSuccess(); // Refresh group details
      setFormData({ payerId: '', payeeId: '', amount: '' }); // Reset form
    } catch (err) {
      setError('Failed to record payment. Please try again.');
    }
  };

  return (
    <form onSubmit={handleSubmit} className="mt-4">
      <h5>Record a Payment</h5>
      {error && <div className="alert alert-danger">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}
      <div className="mb-3">
        <label htmlFor="payerId" className="form-label">Payer</label>
        <select
          className="form-control"
          id="payerId"
          name="payerId"
          value={formData.payerId}
          onChange={handleChange}
          required
        >
          <option value="">Select Payer</option>
          {members.map((member) => (
            <option key={member.userId} value={member.userId}>
              {member.name}
            </option>
          ))}
        </select>
      </div>
      <div className="mb-3">
        <label htmlFor="payeeId" className="form-label">Payee</label>
        <select
          className="form-control"
          id="payeeId"
          name="payeeId"
          value={formData.payeeId}
          onChange={handleChange}
          required
        >
          <option value="">Select Payee</option>
          {members.map((member) => (
            <option key={member.userId} value={member.userId}>
              {member.name}
            </option>
          ))}
        </select>
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
      <button type="submit" className="btn btn-success w-100">Record Payment</button>
    </form>
  );
};

export default PaymentForm;
