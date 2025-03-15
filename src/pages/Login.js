import React, { useState } from 'react';
import { login } from '../services/authService';

const Login = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });
  const [message, setMessage] = useState('');

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
  
    console.log('Sending login data:', formData); // Debug log
  
    try {
      const response = await login(formData); // Call the login service function
      console.log('Login response:', response.data); // Debug log
  
      localStorage.setItem('jwtToken', response.data.token); // Store the token
      setMessage('Login successful! Redirecting to the dashboard...');
      setTimeout(() => {
        window.location.href = '/dashboard'; // Redirect to the dashboard
      }, 1500);
    } catch (error) {
      console.error('Login error:', error.response?.data || error.message); // Debug log
      setMessage(error.response?.data.message || 'Login failed. Please check your credentials.');
    }
  };
  

  return (
    <div className="container mt-5">
      <h2 className="text-center">Login</h2>
      <form className="mx-auto" style={{ maxWidth: '400px' }} onSubmit={handleSubmit}>
        <div className="mb-3">
          <label htmlFor="email" className="form-label">Email</label>
          <input
            type="email"
            className="form-control"
            id="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="password" className="form-label">Password</label>
          <input
            type="password"
            className="form-control"
            id="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            required
          />
        </div>
        <button type="submit" className="btn btn-primary w-100">Login</button>
      </form>
      {message && <div className={`alert mt-3 ${message.includes('successful') ? 'alert-success' : 'alert-danger'}`}>
        {message}
      </div>}
    </div>
  );
};

export default Login;
