import React, { useState } from 'react';
import { addPayment } from '../services/paymentService';

const PaymentForm = ({ groupId, members = [], onSuccess }) => {
  const [formData, setFormData] = useState({
    payerID: '',
    payeeID: '',
    amount: '',
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    setError(''); // Clear previous errors on input change
    setSuccess('');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    try {
      const token = localStorage.getItem('jwtToken');

      // Ensure the structure matches the backend expectation
      const paymentData = {
        PayerID: formData.payerID,  // Match backend property names
        PayeeID: formData.payeeID,
        GroupID: groupId,
        Amount: parseFloat(formData.amount), // Ensure amount is numeric
      };

      console.log('Submitting Payment:', paymentData); // Debug log

      // Call the addPayment service
      await addPayment(paymentData, token);

      setSuccess('Payment recorded successfully!');
      onSuccess(); // Refresh group details
      setFormData({ payerID: '', payeeID: '', amount: '' }); // Reset form fields
    } catch (err) {
      console.error('Error recording payment:', err.response || err.message); // Debug log
      setError(err.response?.data?.message || 'Failed to record payment. Please try again.');
    }
  };

  return (
    <form onSubmit={handleSubmit} className="mt-4">
      <h5>Record a Payment</h5>
      {error && <div className="alert alert-danger">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}
      <div className="mb-3">
        <label htmlFor="payerID" className="form-label">Payer</label>
        <select
          className="form-control"
          id="payerID"
          name="payerID"
          value={formData.payerID}
          onChange={handleChange}
          required
        >
          <option value="">Select Payer</option>
          {members.map((member) => (
            <option key={member.userID} value={member.userID}>
              {member.name}
            </option>
          ))}
        </select>
      </div>
      <div className="mb-3">
        <label htmlFor="payeeID" className="form-label">Payee</label>
        <select
          className="form-control"
          id="payeeID"
          name="payeeID"
          value={formData.payeeID}
          onChange={handleChange}
          required
        >
          <option value="">Select Payee</option>
          {members.map((member) => (
            <option key={member.userID} value={member.userID}>
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
