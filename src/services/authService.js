import axios from 'axios';

const API_URL = 'http://localhost:5220/api/auth';

export const register = async (userData) => {
  return await axios.post(`${API_URL}/register`, userData);
};

export const login = async (credentials) => {
  return await axios.post(`${API_URL}/login`, credentials);
};

export const logout = () => {
  localStorage.removeItem('jwtToken'); // Clear the stored JWT token
};
