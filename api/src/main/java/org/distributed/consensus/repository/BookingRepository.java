package org.distributed.consensus.repository;

import org.distributed.consensus.model.Booking;
import org.springframework.data.jpa.repository.JpaRepository;

public interface BookingRepository extends JpaRepository<Booking, Long> { }
