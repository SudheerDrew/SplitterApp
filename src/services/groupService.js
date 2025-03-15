import axios from 'axios';

const API_URL = 'http://localhost:5220/api/groups';

export const getGroups = async (token) => {
  return await axios.get(API_URL, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
};

export const getGroupDetails = async (groupId, token) => {
  return await axios.get(`${API_URL}/${groupId}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
};

export const createGroup = async (groupData, token) => {
  return await axios.post(API_URL, groupData, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
};
