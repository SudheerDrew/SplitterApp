import React from 'react';
import { Link, useNavigate } from 'react-router-dom';

const Sidebar = () => {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem('jwtToken'); // Clear session
    navigate('/login'); // Redirect to Login
  };

  return (
    <div
      className="d-flex flex-column flex-shrink-0 bg-light"
      style={{ width: '250px', height: '100vh' }}
    >
      <div className="p-3">
        <h4 className="text-center">Expense Splitter</h4>
        <hr />
        <ul className="nav nav-pills flex-column mb-auto">
          <li className="nav-item">
            <Link to="/dashboard" className="nav-link active" aria-current="page">
              Dashboard
            </Link>
          </li>
          <li className="nav-item">
            <Link to="/groups" className="nav-link">
              My Groups
            </Link>
          </li>
        </ul>
        <hr />
        <button
          className="btn btn-danger w-100"
          onClick={handleLogout}
        >
          Logout
        </button>
      </div>
    </div>
  );
};

export default Sidebar;
