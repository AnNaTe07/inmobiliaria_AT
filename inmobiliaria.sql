-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 30-08-2024 a las 04:38:10
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `Id` int(11) NOT NULL,
  `Direccion` varchar(255) NOT NULL,
  `Uso` enum('Comercial','Residencial') NOT NULL,
  `TipoId` int(10) NOT NULL,
  `Ambientes` int(11) NOT NULL,
  `Latitud` double NOT NULL,
  `Longitud` double NOT NULL,
  `Superficie` decimal(10,0) NOT NULL,
  `Precio` decimal(18,2) NOT NULL,
  `IdPropietario` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmueble`
--

INSERT INTO `inmueble` (`Id`, `Direccion`, `Uso`, `TipoId`, `Ambientes`, `Latitud`, `Longitud`, `Superficie`, `Precio`, `IdPropietario`) VALUES
(1, 'Las Heras 222', 'Residencial', 4, 2, 11.11, 22.22, 75, 222222.00, 1),
(2, 'Comechingones 333', 'Comercial', 1, 2, 11.22, 22.11, 80, 333333.10, 2),
(3, 'calle 098', 'Comercial', 1, 3, 25.1, 42.47, 110, 444444.00, 3),
(4, 'mi casa 777', 'Residencial', 4, 5, 111.11, 333.33, 150, 777777.00, 4),
(5, 'Casa Quinta ', 'Residencial', 5, 7, 11.11, 99.99, 223, 999999.00, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilino`
--

CREATE TABLE `inquilino` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Documento` varchar(20) NOT NULL,
  `Telefono` varchar(20) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilino`
--

INSERT INTO `inquilino` (`Id`, `Nombre`, `Apellido`, `Documento`, `Telefono`, `Email`) VALUES
(1, 'Profesor', 'Xavier', '235689745', '26658741235', 'profe@email.com'),
(2, 'Pepe', 'Le Pu', '45235698', '26648521478', 'lepu@email.com'),
(3, 'Doña', 'Cleotilde', '23455698', '26658742365', 'cleotilde@mail.com'),
(4, 'Gallo', 'Claudio', '452356987', '266547896', 'gallo@mail.com'),
(6, 'Ariel', 'Sirenita', '56231245', '266432145698', 'ariel@email.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietario`
--

CREATE TABLE `propietario` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Documento` varchar(20) NOT NULL,
  `Telefono` varchar(20) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `Direccion` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietario`
--

INSERT INTO `propietario` (`Id`, `Nombre`, `Apellido`, `Documento`, `Telefono`, `Email`, `Direccion`) VALUES
(1, 'Ratón', 'Perez', '12564578', '26645689784', 'raton@email.com', 'Su casa 123'),
(2, 'Esteban', 'quito', '45569878', '2664853975', 'esteban@mail.com', 'Lanus 456'),
(3, 'Cindy', 'Entes', '65984578', '2664358962', 'cindy@mail.com', 'Su casa 236'),
(4, 'Pepe', 'Grillo', '98542821', '2665874532', 'pepe@mail.com', 'arbol 999'),
(6, 'Maria', 'Chuzena', '12564552', '266587653289', 'chuzena@mail.com', 'choza 456');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipo`
--

CREATE TABLE `tipo` (
  `Id` int(11) NOT NULL,
  `Descripcion` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipo`
--

INSERT INTO `tipo` (`Id`, `Descripcion`) VALUES
(4, 'Casa'),
(3, 'Departamento'),
(2, 'Depósito'),
(1, 'Local'),
(5, 'Quinta');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_tipo` (`TipoId`);

--
-- Indices de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `DNI` (`Documento`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indices de la tabla `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `DNI` (`Documento`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indices de la tabla `tipo`
--
ALTER TABLE `tipo`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `UC_Descripcion` (`Descripcion`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT de la tabla `tipo`
--
ALTER TABLE `tipo`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `FK_tipo` FOREIGN KEY (`TipoId`) REFERENCES `tipo` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
