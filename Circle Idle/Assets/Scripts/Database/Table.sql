--
-- Table structure for table `datagame`   FOR MySQL DATABASE (NOT MSSQL)
--
CREATE TABLE `datagame` (
  `id` int(11) NOT NULL,
  `timestamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `username` varchar(50) NOT NULL,
  `password` varchar(100) NOT NULL,
  `email` varchar(50) NOT NULL,
  `displayname` varchar(20) NOT NULL DEFAULT 'User',
  `points` int(11) NOT NULL DEFAULT '0',
  `avatar` varchar(20) NOT NULL DEFAULT 'default',
  `characters` varchar(100) NOT NULL DEFAULT '[]',
  `data` varchar(3000) NOT NULL DEFAULT '{}'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Indexes for table `datagame`
--
ALTER TABLE `datagame`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for table `datagame`
--
ALTER TABLE `datagame`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;
COMMIT;