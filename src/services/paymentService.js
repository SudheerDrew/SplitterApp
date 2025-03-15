import axios from 'axios';

const API_URL = 'http://localhost:5220/api/payments';

export const addPayment = async (paymentData, token) => {
  return await axios.post(API_URL, paymentData, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
};
