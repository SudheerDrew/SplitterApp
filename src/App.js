import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Navbar from './components/Navbar';
import Register from './pages/Register';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import GroupDetails from './pages/GroupDetails';
import NotFound from './pages/NotFound';

const App = () => {
  const isLoggedIn = !!localStorage.getItem('jwtToken'); // Check if user is logged in

  return (
    <Router>
      <Navbar />
      <div className="container mt-4">
        <Routes>
          {/* Public Routes */}
          <Route path="/register" element={!isLoggedIn ? <Register /> : <Navigate to="/dashboard" />} />
          <Route path="/login" element={!isLoggedIn ? <Login /> : <Navigate to="/dashboard" />} />

          {/* Protected Routes */}
          <Route path="/dashboard" element={isLoggedIn ? <Dashboard /> : <Navigate to="/login" />} />
          <Route path="/groups/:groupId" element={isLoggedIn ? <GroupDetails /> : <Navigate to="/login" />} />

          {/* Fallback Route */}
          <Route path="*" element={<NotFound />} />
        </Routes>
      </div>
    </Router>
  );
};

export default App;
