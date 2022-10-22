using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid {

    public Vector2 location;
    public Vector2 velocity;
    Vector2 acceleration;
    float size;        // tamaño del boid
    float viewSize;    // tamaño de la vista
    float maxForce;    // la fuerza de atracción
    float maxSpeed;    // velocidad máxima
    float separationWeight;
    float alignmentWeight;
    float cohesionWeight;
    Main main;

    public Boid(float x, float y, Main _main) {
        main = _main;
        acceleration = new Vector2(0, 0);

        
        
        float angle = Random.Range(0,Mathf.PI*2);
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        location = new Vector2(x,y);
    }

    public void Update(List<Boid> boids) {

        viewSize = main.areaSize;
        size = main.cowSize;
        maxSpeed = main.maxSpeed;
        maxForce = main.maxForce;
        separationWeight = main.separationWeight;
        alignmentWeight = main.alignmentWeight;
        cohesionWeight = main.cohesionWeight;

        Flock(boids);
        UpdateLocation();
        Borders();
    }

    void ApplyForce(Vector2 force) {
        
        acceleration += force;
    }

   
    void Flock(List<Boid> boids) {
        Vector2 separationForce = Separate(boids);   // separation
        Vector2 alignmentForce  = Align(boids);      // alineamiento
        Vector2 cohesionForce   = Cohesion(boids);   // Cohesión
        
        separationForce *= separationWeight;
        alignmentForce *= alignmentWeight;
        cohesionForce *= cohesionWeight;
        // se añade el vector de aceleración
        ApplyForce(separationForce);
        ApplyForce(alignmentForce);
        ApplyForce(cohesionForce);
    }

    // aqui se actualiza la localización
    void UpdateLocation() {
        // cambia la velocidad con el tiempo
        velocity += acceleration;
        // y aqui la limito
        velocity = Limit(velocity, maxSpeed);
        location += velocity;
        //se quita la aceleracion para evitar desbordamiento
        acceleration *= 0;
    }

    
    Vector2 Seek(Vector2 target) {
        Vector2 desired = target -location;  // el vector que apunta a donde va a ir la vaca, cual es su objetivo
        
        desired.Normalize();
        desired *= maxSpeed;

        

        
        Vector2 steer = desired - velocity;
        steer = Limit(steer,maxForce);  
        return steer;
    }

    // para evitar choques
    void Borders() {
        if (location.x < -size) location.x = viewSize+size;
        if (location.y < -size) location.y = viewSize+size;
        if (location.x > viewSize+size) location.x = -size;
        if (location.y > viewSize+size) location.y = -size;
    }

    
    Vector2 Separate (List<Boid> boids) {
        float desiredseparation = size;
        Vector2 steer = new Vector2(0, 0);
        int count = 0;
        // se evita que se acerquen demasiado y se buguee
        foreach (var other in boids) {
            
            Vector2 diff = location - other.location;
            float d = Vector2.SqrMagnitude(diff);
            
            if (d > 0 && d < desiredseparation*desiredseparation) {
                
                diff /= d;        
                steer += diff;
                count++;            
            }
        }
       
        if (count > 0) {
            steer /= (float)count;
        }

      
        if (steer.sqrMagnitude > 0) {
          
            steer.Normalize();
            steer *= maxSpeed;
            steer -= velocity;
            steer = Limit(steer, maxForce);
        }
        return steer;
    }

    
    Vector2 Align (List<Boid> boids) {
        float neighbordist = 50;
        Vector2 sum = new Vector2(0, 0);
        int count = 0;
        foreach (var other in boids) {
            float d = Vector2.Distance(location, other.location);
            if ((d > 0) && (d < neighbordist)) {
                sum += other.velocity;
                count++;
            }
        }
        if (count > 0) {
            sum /= (float)count;
         
            sum.Normalize();
            sum *= maxSpeed;
            Vector2 steer = sum - velocity;
            steer = Limit(steer,maxForce);
            return steer;
        } 
        else {
            return new Vector2(0, 0);
        }
    }

   
    Vector2 Cohesion (List<Boid> boids) {
        float neighbordist = 50;
        Vector2 sum = new Vector2(0, 0);  
        int count = 0;
        foreach (var other in boids) {
            float d = Vector2.Distance(location, other.location);
            if ((d > 0) && (d < neighbordist)) {
                sum += other.location;
                count++;
            }
        }
        if (count > 0) {
            sum /= count;
            return Seek(sum);  
        } 
        else {
            return new Vector2(0, 0);
        }
    }

    Vector2 Limit ( Vector2 vector, float max ) {
        if ( vector.sqrMagnitude > max*max )
            return vector.normalized*max;
        else
            return vector;
    }
}
