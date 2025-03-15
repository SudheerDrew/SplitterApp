import React, { useState, useEffect } from 'react';
import { getGroups } from '../services/groupService';
import GroupCard from '../components/GroupCard';
import Sidebar from '../components/Sidebar';

const Dashboard = () => {
  const [groups, setGroups] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchGroups = async () => {
      try {
        const token = localStorage.getItem('jwtToken');
        const response = await getGroups(token);
        setGroups(response.data);
      } catch (err) {
        setError('Failed to load groups. Please try again.');
      } finally {
        setLoading(false);
      }
    };

    fetchGroups();
  }, []);

  return (
    <div className="d-flex">
      <Sidebar />
      <div className="container-fluid mt-3">
        <h2>Dashboard</h2>
        {loading && <p>Loading...</p>}
        {error && <div className="alert alert-danger">{error}</div>}
        {!loading && !error && (
          <div className="row">
            {groups.map((group) => (
              <div key={group.groupID} className="col-md-4">
                <GroupCard group={group} />
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default Dashboard;
