FROM maven:3.9.9-amazoncorretto-21-alpine as MAVEN_BUILD

COPY pom.xml /build/
COPY src /build/src/

WORKDIR /build/

RUN mvn -Dmaven.test.skip=true package -Ptest

FROM openjdk:21

WORKDIR /app

COPY --from=MAVEN_BUILD /build/target/Locking-1.0-SNAPSHOT.jar /app/

ENTRYPOINT ["java", "-jar", "Locking-1.0-SNAPSHOT.jar"]