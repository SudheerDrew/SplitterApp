import axios from 'axios';

const API_URL = 'http://localhost:5220/api/auth';

export const register = async (userData) => {
  return await axios.post(`${API_URL}/register`, userData);
};

// export const login = async (credentials) => {
//   return await axios.post(`${API_URL}/login`, credentials);
// };
export const login = async (credentials) => {
  try {
    const response = await axios.post(`${API_URL}/login`, credentials);
    console.log('Login Response:', response.data); // Log here
    return response;
  } catch (err) {
    console.error('Login Error:', err.response || err);
    throw err;
  }
};


export const logout = () => {
  localStorage.removeItem('jwtToken'); // Clear the stored JWT token
};
