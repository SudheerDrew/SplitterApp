import React from 'react';
import { Link } from 'react-router-dom';

const Sidebar = () => {
  return (
    <div className="d-flex flex-column flex-shrink-0 bg-light" style={{ width: '250px', height: '100vh' }}>
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
              Groups
            </Link>
          </li>
          <li className="nav-item">
            <Link to="/profile" className="nav-link">
              Profile
            </Link>
          </li>
          <li className="nav-item">
            <Link to="/settings" className="nav-link">
              Settings
            </Link>
          </li>
        </ul>
        <hr />
        <button
          className="btn btn-danger w-100"
          onClick={() => {
            localStorage.removeItem('jwtToken'); // Clear session
            window.location.href = '/login'; // Redirect to Login
          }}
        >
          Logout
        </button>
      </div>
    </div>
  );
};

export default Sidebar;
