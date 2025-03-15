import React from 'react';
import { useNavigate } from 'react-router-dom';

const GroupCard = ({ group }) => {
  const navigate = useNavigate();

  return (
    <div className="card mb-3 shadow-sm">
      <div className="card-body">
        <h5 className="card-title">{group.groupName}</h5>
        <p className="card-text">
          Total Expenses: ₹{group.totalExpenses || 0} <br />
          Members: {group.membersCount || 0}
        </p>
        <button
          className="btn btn-primary"
          onClick={() => navigate(`/groups/${group.groupID}`)}
        >
          View Details
        </button>
      </div>
    </div>
  );
};

export default GroupCard;
