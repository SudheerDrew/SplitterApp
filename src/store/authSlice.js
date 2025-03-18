import { createSlice } from '@reduxjs/toolkit';

const authSlice = createSlice({
  name: 'auth',
  initialState: {
    user: null,
    token: localStorage.getItem('jwtToken') || null, // Automatically load token from storage
  },
  reducers: {
    setUser(state, action) {
      state.user = action.payload.user;
      state.token = action.payload.token;
      localStorage.setItem('jwtToken', action.payload.token); // Persist token
    },
    clearUser(state) {
      state.user = null;
      state.token = null;
      localStorage.removeItem('jwtToken'); // Clear token from storage
    },
  },
});

export const { setUser, clearUser } = authSlice.actions;
export default authSlice.reducer;
