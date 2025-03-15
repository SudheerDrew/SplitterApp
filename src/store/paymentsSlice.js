import { createSlice } from '@reduxjs/toolkit';

const paymentsSlice = createSlice({
  name: 'payments',
  initialState: {
    payments: [],
  },
  reducers: {
    setPayments(state, action) {
      state.payments = action.payload;
    },
    addPayment(state, action) {
      state.payments.push(action.payload);
    },
  },
});

export const { setPayments, addPayment } = paymentsSlice.actions;
export default paymentsSlice.reducer;
