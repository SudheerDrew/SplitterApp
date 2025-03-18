import { createSlice } from '@reduxjs/toolkit';

const expensesSlice = createSlice({
  name: 'expenses',
  initialState: {
    expenses: [],
  },
  reducers: {
    setExpenses(state, action) {
      state.expenses = action.payload; // Set entire expenses list
    },
    addExpense(state, action) {
      state.expenses.push(action.payload); // Add a single expense
    },
  },
});

export const { setExpenses, addExpense } = expensesSlice.actions;
export default expensesSlice.reducer;
