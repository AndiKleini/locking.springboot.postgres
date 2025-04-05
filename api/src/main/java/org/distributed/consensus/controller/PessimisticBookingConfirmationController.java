package org.distributed.consensus.controller;

import org.distributed.consensus.model.BookingConfirmation;
import org.distributed.consensus.repository.BookingConfirmationRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/pessimistic")
public class PessimisticBookingConfirmationController {

    @Autowired
    BookingConfirmationRepository bookingRepository;

    @PostMapping("/bookingconfirmation")
    public ResponseEntity<BookingConfirmation> createBooking(@RequestBody BookingConfirmation booking) {
        try {
            BookingConfirmation _bookingConfirmation = bookingRepository.save(new BookingConfirmation(booking.getBookingId()));
            return new ResponseEntity<>(_bookingConfirmation, HttpStatus.CREATED);
        } catch (Exception e) {
            return new ResponseEntity<>(null, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }
}
