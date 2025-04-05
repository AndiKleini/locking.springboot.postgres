package org.distributed.consensus.model;

import java.sql.Date;
import jakarta.persistence.*;

@Entity
@Table(name = "bookings")
public class Booking {

    @Id
    @GeneratedValue(strategy = GenerationType.AUTO)
    private long id;

    @Column(name = "name")
    private String name;

    @Column(name = "roomId")
    private long roomId;

    @Column(name = "start")
    private Date start;

    @Column(name = "finish")
    private Date finish;

    public Booking(String name, long roomId, Date from, Date end) {
        this.name = name;
        this.roomId = roomId;
        this.start = from;
        this.finish = end;
    }

    public Booking() {

    }

    public String getName() {
        return name;
    }

    public long getId() {
        return id;
    }

    public long getRoomId() {
        return roomId;
    }

    public Date getStart() {
        return start;
    }

    public Date getFinish() {
        return finish;
    }
}
