package org.distributed.consensus.controller;

import org.distributed.consensus.model.Booking;
import org.distributed.consensus.model.BookingPolicies;
import org.distributed.consensus.repository.BookingRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;

@RestController
@RequestMapping("/optimistic")
public class OptimisticBookingController {

    @Autowired
    BookingRepository bookingRepository;

    @GetMapping("/booking/{id}")
    public ResponseEntity<Booking> getBooking(@PathVariable long id) {
        Optional<Booking> booking = this.bookingRepository.findById(id);
        return new ResponseEntity<>(booking.orElse(null), booking.isEmpty() ? HttpStatus.NOT_FOUND : HttpStatus.OK);
    }

    @GetMapping("/booking")
    public ResponseEntity<List<Booking>> getBooking() {
        List<Booking> allBookings = this.bookingRepository.findAll();
        return new ResponseEntity<>(allBookings, allBookings.isEmpty() ? HttpStatus.NO_CONTENT : HttpStatus.OK );
    }

    @PostMapping("/booking")
    public ResponseEntity<Booking> createBooking(@RequestBody Booking booking) {

        // unfortunately we cannot guarantee that all locations which might
        // evaluate to false here are not somehow offered to our customers
        // (e.g.: shared links in social medias, clients that are working
        // with old data ... )
        // Therefore this check is crucial
        if (!BookingPolicies.canBook(booking)) {
            return new ResponseEntity<>(null, HttpStatus.CONFLICT);
        }

        try {
            Booking _booking = bookingRepository.save(new Booking(booking.getName(), booking.getRoomId(), booking.getStart(), booking.getFinish()));
            return new ResponseEntity<>(_booking, HttpStatus.CREATED);
        } catch (Exception e) {
            return new ResponseEntity<>(null, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    @DeleteMapping("/booking/{id}")
    public ResponseEntity<HttpStatus> deleteBooking(@PathVariable("id") long id) {
        try {
            bookingRepository.deleteById(id);
            return new ResponseEntity<>(HttpStatus.NO_CONTENT);
        } catch (Exception e) {
            return new ResponseEntity<>(HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }
}
