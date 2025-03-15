import axios from 'axios';

const API_URL = 'http://localhost:5220/api/expenses';

export const addExpense = async (expenseData, token) => {
  return await axios.post(API_URL, expenseData, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
};
