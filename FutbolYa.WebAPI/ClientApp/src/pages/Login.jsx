import React, { useState } from 'react';
import { login } from '../api/auth';
import { useNavigate } from 'react-router-dom';
import '../assets/styles/login.css';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [mostrarPassword, setMostrarPassword] = useState(false);
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const response = await login(email, password);
      const { token, usuario } = response.data;

      if (token) {
        localStorage.setItem('token', token);
        localStorage.setItem('usuario', JSON.stringify(usuario));
        navigate('/home');
      } else {
        alert('Inicio de sesión sin token recibido.');
      }
    } catch (error) {
      alert('Error al iniciar sesión. Verificá tus datos.');
      console.error(error);
    }
  };

  const irARegistro = () => {
    navigate('/register');
  };

  return (
    <div className="login-background">
      <div className="login-box">
        <div className="login-header">
          <img src="/IconoFYa.jpeg" alt="Logo FútbolYa" />
          <h2>LOGIN</h2>
        </div>
        <form onSubmit={handleLogin}>
          <input
            type="email"
            value={email}
            onChange={e => setEmail(e.target.value)}
            placeholder="Usuario"
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
          <div className="forgot-password">
            <a href="#">¿Olvidaste tu contraseña?</a>
          </div>
          <button type="submit">Iniciar Sesión</button>
        </form>
        <button className="buttonC" onClick={irARegistro}>
          Crear Cuenta
        </button>
      </div>
    </div>
  );
};

export default Login;
