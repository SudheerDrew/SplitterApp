import React from 'react';
import { useNavigate } from 'react-router-dom';

const GroupCard = ({ group }) => {
  const navigate = useNavigate();

  return (
    <div className="card">
      <div className="card-body">
        <h5 className="card-title">{group.groupName}</h5>
        <p className="card-text">Members: {group.members?.length || 0}</p>
        <button
          className="btn btn-primary"
          onClick={() => {
            console.log('Navigating to groupID:', group.groupID); // Debugging log
            navigate(`/groups/${group.groupID}`);
          }}
        >
          View Details
        </button>
      </div>
    </div>
  );
};

export default GroupCard;
