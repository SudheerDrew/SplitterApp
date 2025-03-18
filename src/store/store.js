import { configureStore } from '@reduxjs/toolkit';
import authReducer from './authSlice';
import groupsReducer from './groupsSlice';
import expensesReducer from './expensesSlice';
import paymentsReducer from './paymentsSlice';

const store = configureStore({
  reducer: {
    auth: authReducer, // Manages user authentication and token
    groups: groupsReducer, // Manages groups and related data
    expenses: expensesReducer, // Manages expenses within groups
    payments: paymentsReducer, // Manages payments within groups
  },
});

export default store;
