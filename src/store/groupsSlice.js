import { createSlice } from '@reduxjs/toolkit';

const groupsSlice = createSlice({
  name: 'groups',
  initialState: {
    groups: [],
  },
  reducers: {
    setGroups(state, action) {
      state.groups = action.payload; // Set entire groups list
    },
    addGroup(state, action) {
      state.groups.push(action.payload); // Add a single group
    },
  },
});

export const { setGroups, addGroup } = groupsSlice.actions;
export default groupsSlice.reducer;
