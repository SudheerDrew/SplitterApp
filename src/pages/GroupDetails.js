import React, { useEffect, useState, useCallback } from 'react';
import { getGroupDetails } from '../services/groupService';
import GroupCard from '../components/GroupCard';
import ExpenseForm from '../components/ExpenseForm';
import PaymentForm from '../components/PaymentForm';
import BalanceSummary from '../components/BalanceSummary';

const GroupDetails = ({ groupId }) => {
  const [group, setGroup] = useState(null);
  const [error, setError] = useState('');

  const fetchGroupDetails = useCallback(async () => {
    try {
      const token = localStorage.getItem('jwtToken');
      const response = await getGroupDetails(groupId, token);
      setGroup(response.data);
    } catch (err) {
      setError('Failed to load group details. Please try again.');
    }
  }, [groupId]); // <-- Add groupId as a dependency

  useEffect(() => {
    fetchGroupDetails();
  }, [fetchGroupDetails]); // <-- Add fetchGroupDetails as a dependency

  return (
    <div className="container mt-5">
      <h2>Group Details</h2>
      {error && <div className="alert alert-danger">{error}</div>}
      {group && (
        <>
          <h3>{group.groupName}</h3>
          <GroupCard group={group} />
          <BalanceSummary members={group.members} />
          <ExpenseForm groupId={groupId} onSuccess={fetchGroupDetails} />
          <PaymentForm groupId={groupId} members={group.members} onSuccess={fetchGroupDetails} />
        </>
      )}
    </div>
  );
};

export default GroupDetails;
