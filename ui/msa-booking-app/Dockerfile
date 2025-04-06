# Use the official Node.js image for building Angular
FROM node:18

# Set the working directory inside the container
WORKDIR /app

# Copy package.json and package-lock.json for dependency installation
COPY package*.json ./

# Install Angular dependencies
RUN npm install

# Copy the application code into the container
COPY . .

# Build the Angular app
RUN npm run build

# Use Nginx to serve the built Angular app
FROM nginx:alpine
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=0 /app/dist /usr/share/nginx/html

# Expose port for Angular app
EXPOSE 80

# Start the Nginx server
CMD ["nginx", "-g", "daemon off;"]