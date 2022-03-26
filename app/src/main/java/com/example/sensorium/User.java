package com.example.sensorium;

import android.util.Log;

import com.google.firebase.database.IgnoreExtraProperties;

import java.util.HashMap;

import io.neuos.NeuosSDK;

@IgnoreExtraProperties
public class User {

    //public String userId;
    public float zone;
    public float focus;
    public float enjoyment;
    public float heart;
    public float motion;

    public User() {
        // Default constructor required for calls to DataSnapshot.getValue(User.class)
    }

    public User(float zone, float focus, float enjoyment, float heart, float motion) {
        this.zone = zone;
        this.focus = focus;
        this.enjoyment = enjoyment;
        this.heart = heart;
        this.motion = motion;
    }

    public void updateUser(String key, float value) {
        String finalKey = "";
        switch (key) {
            case NeuosSDK.PredictionValues.ZONE_STATE:
                this.zone = value;
                finalKey = "zone";
                break;
            case NeuosSDK.PredictionValues.FOCUS_STATE:
                this.focus = value;
                finalKey = "focus";
                break;
            case NeuosSDK.PredictionValues.ENJOYMENT_STATE:
                this.enjoyment = value;
                finalKey = "enjoyment";
                break;
            case NeuosSDK.PredictionValues.HEART_RATE:
                this.heart = value;
                finalKey = "heart";
                break;
            case NeuosSDK.PredictionValues.AVG_MOTION:
                this.motion = value;
                finalKey = "motion";
                break;
            default:
                break;
        }
        //return new HashMap<String, Object>(finalKey, value);


    }


}
