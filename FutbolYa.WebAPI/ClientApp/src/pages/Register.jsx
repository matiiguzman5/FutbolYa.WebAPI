import React, { useState } from 'react';
import { register } from '../api/auth';
import { useNavigate } from 'react-router-dom';
import '../assets/styles/login.css';

const Register = () => {
  const [nombre, setNombre] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [mostrarPassword, setMostrarPassword] = useState(false);
  const navigate = useNavigate();

  const handleRegister = async (e) => {
    e.preventDefault();
    try {
      await register(nombre, email, password);
      alert('Registro exitoso. Ahora podés iniciar sesión.');
      navigate('/login');
    } catch (error) {
      alert('Error al registrarse. Verificá los datos.');
      console.error(error);
    }
  };

  return (
    <div className="login-background">
      <div className="login-box">
        <div className="login-header">
          <img src="/IconoFYa.jpeg" alt="Logo FútbolYa" />
          <h2>REGISTRO</h2>
        </div>
        <form onSubmit={handleRegister}>
          <input
            type="text"
            value={nombre}
            onChange={e => setNombre(e.target.value)}
            placeholder="Nombre"
            required
          />
          <input
            type="email"
            value={email}
            onChange={e => setEmail(e.target.value)}
            placeholder="Correo"
            required
          />
          <div className="password-container">
            <input
              type={mostrarPassword ? 'text' : 'password'}
              value={password}
              onChange={e => setPassword(e.target.value)}
              placeholder="Contraseña"
              required
            />
            <span
              className="toggle-password"
              onClick={() => setMostrarPassword(!mostrarPassword)}
            >
              {mostrarPassword ? '🙈' : '👁️'}
            </span>
          </div>
          <button type="submit">Registrarme</button>
        </form>
        <button className="buttonC" onClick={() => navigate('/login')}>
          Volver al Login
        </button>
      </div>
    </div>
  );
};

export default Register;
