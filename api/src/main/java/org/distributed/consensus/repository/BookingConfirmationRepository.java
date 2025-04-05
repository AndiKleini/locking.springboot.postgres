package org.distributed.consensus.repository;

import org.distributed.consensus.model.BookingConfirmation;
import org.springframework.data.jpa.repository.JpaRepository;

public interface BookingConfirmationRepository extends JpaRepository<BookingConfirmation, Long> { }
