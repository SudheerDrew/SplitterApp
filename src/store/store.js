import { configureStore } from '@reduxjs/toolkit';
import authReducer from './authSlice';
import groupsReducer from './groupsSlice';
import expensesReducer from './expensesSlice';
import paymentsReducer from './paymentsSlice';

const store = configureStore({
  reducer: {
    auth: authReducer,
    groups: groupsReducer,
    expenses: expensesReducer,
    payments: paymentsReducer,
  },
});

export default store;
