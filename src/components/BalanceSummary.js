import React from 'react';

const BalanceSummary = ({ members }) => {
  return (
    <div className="mt-4">
      <h5>Balance Summary</h5>
      {members && members.length > 0 ? (
        <ul className="list-group">
          {members.map((member) => (
            <li key={member.userId} className="list-group-item">
              <strong>{member.name}</strong> owes a total of ₹
              {Math.abs(member.balance) || 0}{' '}
              {member.balance > 0 ? 'to the group' : 'and is owed by the group'}
            </li>
          ))}
        </ul>
      ) : (
        <p className="text-muted">No balance information available.</p>
      )}
    </div>
  );
};

export default BalanceSummary;
