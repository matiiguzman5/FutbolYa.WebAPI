// src/App.jsx
import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import Home from './pages/Home';
import './assets/styles/global.css';
import Perfil from './pages/Perfil';
import AdminUsuarios from './pages/AdminUsuarios';


const App = () => (
  <Router>
    <Routes>
      <Route path="/" element={<Login />} />
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route path="/home" element={<Home />} />
      <Route path="/perfil" element={<Perfil />} />
      <Route path="/admin-usuarios" element={<AdminUsuarios />} />
    </Routes>
  </Router>
);

export default App;
