import React, { useEffect, useState } from 'react';
import '../assets/styles/home.css';
import { Link } from 'react-router-dom';

const Home = () => {
  const [partidos, setPartidos] = useState([]);
  const [paginaActual, setPaginaActual] = useState(1);
  const [busqueda, setBusqueda] = useState('');
  const partidosPorPagina = 4;

  const usuario = JSON.parse(localStorage.getItem('usuario'));
  const esAdmin = usuario?.rol === 'administrador';

  useEffect(() => {
    const fetchPartidos = async () => {
      try {
        const res = await fetch('https://localhost:7055/api/partidos');
        const data = await res.json();
        setPartidos(data);
      } catch (error) {
        console.error('Error al obtener partidos:', error);
      }
    };

    fetchPartidos();
  }, []);

  const partidosFiltrados = partidos.filter(p =>
    p.nombre?.toLowerCase().includes(busqueda.toLowerCase()) ||
    p.ubicacion?.toLowerCase().includes(busqueda.toLowerCase())
  );

  const totalPaginas = Math.ceil(partidosFiltrados.length / partidosPorPagina);
  const indexInicio = (paginaActual - 1) * partidosPorPagina;
  const indexFin = indexInicio + partidosPorPagina;
  const partidosPaginados = partidosFiltrados.slice(indexInicio, indexFin);

  return (
    <div className="home-wrapper">
      <div className="home-content">
        <header className="home-header">
          <img src="/IconoFYa.jpeg" alt="Logo" className="logo" />
          <nav>
            <a href="#">Cambiar</a>
            <a href="#">Sedes</a>
            <Link to="/perfil">Perfil</Link>
            {esAdmin && <Link to="/admin-usuarios">Administrar</Link>}
          </nav>
        </header>

        <div className="home-search">
          <div className="search-container">
            <input
              type="text"
              placeholder="Buscar..."
              value={busqueda}
              onChange={(e) => setBusqueda(e.target.value)}
            />
          </div>
        </div>

        <div className="partidos-grid">
          {partidosPaginados.map((p) => (
            <div key={p.id} className="partido-card">
              <img src="/cancha.jpg" alt={p.nombre} />
              <div className="tipo-cancha">{p.tipoCancha}</div>
              <div className="info">
                <strong>{p.nombre}</strong>
                <p>{p.ubicacion}</p>
                <p>{new Date(p.fecha).toLocaleString('es-AR')}</p>
                <p>Faltan {(p?.maximoJugadores ?? 0) - (Array.isArray(p?.jugadores) ? p.jugadores.length : 0)} jugadores</p>
              </div>
            </div>
          ))}
        </div>

        <div className="paginacion">
          {[...Array(totalPaginas)].map((_, i) => (
            <button
              key={i}
              className={paginaActual === i + 1 ? 'activo' : ''}
              onClick={() => setPaginaActual(i + 1)}
            >
              {i + 1}
            </button>
          ))}
        </div>
      </div>

      <footer className="home-footer">
        <p>© 2024 FutbolYa</p>
        <div className="footer-links">
          <a href="#">CONTACTO</a>
          <a href="#">AYUDA</a>
          <a href="#">SOBRE NOSOTROS</a>
          <a href="#">METODOS DE PAGO</a>
        </div>
        <div className="footer-icons">
          <span>📘</span>
          <span>🐦</span>
          <span>📸</span>
        </div>
      </footer>
    </div>
  );
};

export default Home;
