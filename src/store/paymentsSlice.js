import { createSlice } from '@reduxjs/toolkit';

const paymentsSlice = createSlice({
  name: 'payments',
  initialState: {
    payments: [],
  },
  reducers: {
    setPayments(state, action) {
      state.payments = action.payload; // Set entire payments list
    },
    addPayment(state, action) {
      state.payments.push(action.payload); // Add a single payment
    },
  },
});

export const { setPayments, addPayment } = paymentsSlice.actions;
export default paymentsSlice.reducer;
