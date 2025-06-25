// src/api/auth.js
import axios from 'axios';

const API_URL = 'https://localhost:7055/api'; // Tu API de Visual Studio

export const login = async (email, password) => {
  return axios.post(`${API_URL}/auth/login`, {
    correo: email,
    contraseña: password
  });
};

export const register = async (nombre, email, password) => {
  return axios.post(`${API_URL}/auth/registro`, { 
    nombre: nombre,
    correo: email,
    contraseña: password
  });
};
