import React from 'react';

const BalanceSummary = ({ balanceSummary }) => {
  return (
    <div className="mt-4">
      <h5>Balance Summary</h5>
      {balanceSummary && balanceSummary.length > 0 ? (
        <ul className="list-group">
          {balanceSummary.map((member) => (
            <li key={member.userID} className="list-group-item">
              <strong>{member.name}</strong> owes ₹
              {Math.abs(member.owes)}{' '}
              {member.owes > 0
                ? `to the group`
                : `and is owed ₹${Math.abs(member.owedByGroup)} by the group`}
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
