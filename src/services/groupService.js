import axios from 'axios';

const API_URL = 'http://localhost:5220/api/groups';

// Fetch all groups
export const getGroups = async (token) => {
  try {
    const response = await axios.get(API_URL, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data; // GroupDto array
  } catch (err) {
    console.error('Error fetching groups from backend:', err.response || err);
    throw err.response?.data || { message: 'An unexpected error occurred.' };
  }
};

// Fetch group details by groupId
export const getGroupDetails = async (groupId, token) => {
  try {
    const response = await axios.get(`${API_URL}/${groupId}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data; // Single GroupDto
  } catch (err) {
    console.error('Error fetching group details:', err.response || err);
    throw err.response?.data || { message: 'An unexpected error occurred.' };
  }
};

// Create a new group
export const createGroup = async (groupData, token) => {
  try {
    console.log('Sending Group Data to Backend:', groupData); // Debugging log
    const response = await axios.post(API_URL, groupData, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data; // Newly created GroupDto
  } catch (err) {
    console.error('Error creating group:', err.response || err);
    throw err.response?.data || { message: 'An unexpected error occurred.' };
  }
};

// Add members to a group
export const addMembers = async (groupId, userIds, token) => {
  try {
    console.log('Adding Members:', { groupId, userIds }); // Debugging log
    const response = await axios.post(`http://localhost:5220/api/groups/${groupId}/add-members`, userIds, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data; // Success message or added members
  } catch (err) {
    console.error('Error adding members to group:', err.response || err);
    throw err.response?.data || { message: 'An unexpected error occurred.' };
  }
};
