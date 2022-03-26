package com.example.sensorium;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.appcompat.app.AppCompatActivity;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.ServiceConnection;
import android.content.pm.ActivityInfo;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.content.pm.ServiceInfo;
import android.os.Bundle;
import android.os.IBinder;
import android.os.RemoteException;
import android.util.Log;
import android.view.View;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import io.neuos.INeuosSdk;
import io.neuos.INeuosSdkListener;
import io.neuos.NeuosSDK;
import io.neuos.NeuosQAProperties;



public class MainActivity extends AppCompatActivity {

    private String TAG = "ARCTOP";
    private String API_KEY = "qfxlwdXjTlgpuJQkW";
    TextView zonetxt;
    TextView focustxt;
    TextView enjoymenttxt;
    TextView hearttxt;
    TextView avgtxt;

    ProgressBar pb;
    TextView statustxt;
    EditText uidtxt;
    String userId;

    int counter;

    private ServiceConnection mConnection;
    private INeuosSdk mService;
    private DevicePairingBroadcastReceiver devicePairingBroadcastReceiver;

    private DatabaseReference mDatabase;
    private User userStatus;


    private final ActivityResultLauncher<String> requestPermissionLauncher =
            registerForActivityResult(new ActivityResultContracts.RequestPermission(), isGranted -> {
                if (isGranted) {
                    // Permission is granted. Continue the action or workflow in your app
                    //doBindService();
                } else {
                    // Explain to the user that the feature is unavailable because the
                    // features requires a permission that the user has denied. At the
                    // same time, respect the user's decision. Don't link to system
                    // settings in an effort to convince the user to change their
                    // decision.
                }
            });

    // Activity launcher for QA result
    private final ActivityResultLauncher<Intent> qaResultLauncher = registerForActivityResult(
            new ActivityResultContracts.StartActivityForResult(),
            result -> {
                if (result.getResultCode() != Activity.RESULT_OK) {
                    // Here you should work with your user to get his headband working
                    Toast.makeText(this,"QA Failed!", Toast.LENGTH_SHORT).show();
                    // Or terminate the session
                    try {
                        mService.terminateSessionQaFailed();
                    } catch (RemoteException e) {
                        e.printStackTrace();
                    }
                }
                else{
                    // we are good to go, launch the game
                    try {
                        mService.startPredictionSession(NeuosSDK.Predictions.ZONE);
                    } catch (RemoteException e) {
                        Toast.makeText(this,"Error with running prediction", Toast.LENGTH_SHORT).show();
                        e.printStackTrace();

                    }
                }
            });


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        requestPermissionLauncher.launch(
                NeuosSDK.NEUOS_PERMISSION);
        zonetxt = findViewById(R.id.zone_state_txt);
        focustxt = findViewById(R.id.focus_state_txt);
        enjoymenttxt = findViewById(R.id.enjoyment_state_txt);
        hearttxt = findViewById(R.id.heart_rate_txt);
        avgtxt = findViewById(R.id.avg_motion_txt);
        pb = findViewById(R.id.progressBar);
        statustxt = findViewById(R.id.statustxt);
        uidtxt = findViewById(R.id.uidtxt);

//        FirebaseDatabase database = FirebaseDatabase.getInstance();
//        mDatabase = FirebaseDatabase.getInstance().getReference("users");
        FirebaseDatabase database = FirebaseDatabase.getInstance();
        DatabaseReference myRef = database.getReference("test");

        myRef.setValue("Hello, World!");

//        DatabaseReference myRef = database.getReference("message");
//
//        myRef.setValue("Hello, World 2!");

        final INeuosSdkListener mCallback = new INeuosSdkListener.Stub() {
            @Override
            public void onConnectionChanged(int previousConnection, int currentConnection) throws RemoteException {
                // device is connected
                if (currentConnection == NeuosSDK.ConnectionState.CONNECTED) {

//                    // run the QA Activity
//                    Intent explicit = getExplicitIntent(new Intent(NeuosSDK.NEUOS_QA_SCREEN));
//                    if (explicit == null){
//                        Log.d(TAG, "Cannot find QA activity");
//                        return;
//                    }
//                    explicit.putExtra(NeuosQAProperties.STAND_ALONE , true);
//                    explicit.putExtra(NeuosQAProperties.TASK_PROPERTIES ,
//                            new NeuosQAProperties(NeuosQAProperties.Quality.Normal , NeuosQAProperties.INFINITE_TIMEOUT));
//                    qaResultLauncher.launch(explicit);
                    mService.startPredictionSession(NeuosSDK.Predictions.ZONE);

                }
            }

            @SuppressLint("SetTextI18n")
            @Override
            public void onValueChanged(String key, float value) throws RemoteException {

                runOnUiThread(new Runnable() {

                    @Override
                    public void run() {
                        if (pb.getVisibility() == View.VISIBLE) {
                            pb.setVisibility(View.GONE);
                        }
                        counter += 1;

                        if (value != -1) {
                            // Stuff that updates the UI
                            switch (key) {
                                case NeuosSDK.PredictionValues.ZONE_STATE:
                                    zonetxt.setText("Zone State: " + value);
                                    Log.i(TAG, "zone: " + value);

                                    break;
                                case NeuosSDK.PredictionValues.FOCUS_STATE:
                                    focustxt.setText("Focus State: " + value);
                                    Log.i(TAG, "focus: " + value);
                                    break;
                                case NeuosSDK.PredictionValues.ENJOYMENT_STATE:
                                    enjoymenttxt.setText("Enjoyment State: " + value);
                                    Log.i(TAG, "enjoyment: " + value);
                                    break;
                                case NeuosSDK.PredictionValues.HEART_RATE:
                                    hearttxt.setText("Heart rate: " + value);
                                    Log.i(TAG, "heart: " + value);

                                    break;
                                case NeuosSDK.PredictionValues.AVG_MOTION:
                                    avgtxt.setText("Average Motion: " + value);
                                    //Log.i(TAG, "motion: " + value);
                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                });
                //updateDatabase(key, value);

            }

            @Override
            public void onQAStatus(boolean passed, int type) throws RemoteException {

            }

            @Override
            public void onSessionComplete() throws RemoteException {

            }

            @Override
            public void onError(int errorCode, String message) throws RemoteException {

            }
            // Implement your interface here
        };

        mConnection = new ServiceConnection() {
            public void onServiceConnected(ComponentName className,
                                           IBinder service) {
                Log.d(TAG, "Attached.");
                // Once connected, you can cast the binder object
                // into the proper type
                mService = INeuosSdk.Stub.asInterface(service);
                //And start interacting with the service.
                try {
                    // Initialize the service API with your API key
                    int response = mService.initializeNeuos(API_KEY);
                    if ( response == NeuosSDK.ResponseCodes.SUCCESS){
                        // Register for service callbacks
                        Toast toast = Toast.makeText(getApplicationContext(), "connection success!!", Toast.LENGTH_SHORT);
                        toast.show();

                        response = mService.registerSDKCallback(mCallback);
                        if ( response == NeuosSDK.ResponseCodes.SUCCESS){
                            // Service is ready to work with

                            int status = mService.getUserLoginStatus();
                            switch (status){
                                case NeuosSDK.LoginStatus.LOGGED_IN:{
                                    Log.i(TAG, "login: Logged In");
                                    break;
                                }
                                case NeuosSDK.LoginStatus.NOT_LOGGED_IN:{
                                    Log.i(TAG, "login: Not Logged In");
                                    Intent loginIntent = new Intent(NeuosSDK.NEUOS_LOGIN);
                                    startActivity(loginIntent);
                                    break;
                                }
                            }
                            checkCalibrationStatus();




                        }
                    }
                } catch (Exception e) {
                    Log.e(TAG, e.getLocalizedMessage());
                }
            }

            public void onServiceDisconnected(ComponentName className) {
                // This will be called when an Unexpected disconnection happens.
                Log.d(TAG, "Detached.");
            }
        };



    }

    public void updateDatabase(String key, float value) {
        userStatus.updateUser(key, value);
        if (value > 0 && counter % 25 == 0) {
            // might want to update instead of add
            mDatabase.child("users").child(userId).setValue(userStatus);
            //Map<String, Object> postValues = new HashMap<String,Object>();

            //mDatabase.child("users").child(userId).childUpdate
        }
    }

    @SuppressLint("SetTextI18n")
    public void onScanClick(View view) {
        if (uidtxt.getText().length() < 1) {
            Toast.makeText(MainActivity.this, "Please insert your magic leap id before starting!", Toast.LENGTH_SHORT).show();
        } else {
            userId = String.valueOf(uidtxt.getText());
            Toast.makeText(MainActivity.this, "Scanning user: " + userId, Toast.LENGTH_SHORT).show();
            uidtxt.setVisibility(View.GONE);
            statustxt.setText("Magic Leap Id: " + userId);
            view.setVisibility(View.GONE);
            pb.setVisibility(View.VISIBLE);
            pb.setIndeterminate(true);

            userStatus = new User(0, 2, 0, 0, 0);
//            mDatabase.setValue("hello");
//            mDatabase.child(userId).setValue(userStatus);
            doBindService();
        }
    }

    void doBindService() {

        try {
            // Create an intent based on the class name
            Intent serviceIntent = new Intent(INeuosSdk.class.getName());
            // Use package manager to find intent reciever
            List<ResolveInfo> matches=getPackageManager()
                    .queryIntentServices(serviceIntent, 0);
            if (matches.size() == 0) {
                Log.d(TAG, "Cannot find a matching service!");
                Toast.makeText(this, "Cannot find a matching service!",
                        Toast.LENGTH_LONG).show();
            }
            else if (matches.size() > 1) {
                // This is really just a sanity check
                // and should never occur in a real life scenario
                Log.d(TAG, "Found multiple matching services!");
                Toast.makeText(this, "Found multiple matching services!",
                        Toast.LENGTH_LONG).show();
            }
            else {
                // Create an explicit intent
                Intent explicit=new Intent(serviceIntent);
                ServiceInfo svcInfo=matches.get(0).serviceInfo;
                ComponentName cn=new ComponentName(svcInfo.applicationInfo.packageName,
                        svcInfo.name);
                explicit.setComponent(cn);
                // Bind using AUTO_CREATE
                if (bindService(explicit, mConnection,  BIND_AUTO_CREATE)){
                    Log.d(TAG, "Bound to Neuos Service");
                } else {
                    Log.d(TAG, "Failed to bind to Neuos Service");
                }
            }
        } catch (Exception e) {
            pb.setIndeterminate(false);

            Log.e(TAG, "can't bind to NeuosService, check permission in Manifest");
        }
    }

    private void checkCalibrationStatus(){
        try {
            int calibrationStatus = mService.checkUserCalibrationStatus();
            Log.i(TAG, "onUserCalibrationStatus: " + calibrationStatus);
            switch (calibrationStatus) {
                case NeuosSDK.UserCalibrationStatus.NEEDS_CALIBRATION:
                    Log.i(TAG, "User is not calibrated. Cannot perform realtime stream");
                    Intent activityIntent = getExplicitIntent(new Intent(NeuosSDK.NEUOS_CALIBRATION));
                    startActivity(activityIntent);
                    break;
                case NeuosSDK.UserCalibrationStatus.CALIBRATION_DONE:
                    Log.i(TAG, "Models for this user have yet to be processed");
                    break;
                case NeuosSDK.UserCalibrationStatus.MODELS_AVAILABLE:
                    connectToDevice();
//                    mPostConnection = () -> {
//                        // Start a session
//                        if (startSession() == NeuosSDK.ResponseCodes.SUCCESS) {
//                            // once started successfully, launch QA screen
//                            launchQAScreen();
//                        }
//                    };
                    break;
            }
        } catch (RemoteException e) {
            Log.e(TAG, e.getLocalizedMessage());
        }
    }


    private class DevicePairingBroadcastReceiver extends BroadcastReceiver {
        @Override
        public void onReceive(Context context, Intent intent) {
            String address = intent.getStringExtra(NeuosSDK.DEVICE_ADDRESS_EXTRA);
            Log.d(TAG, "Connection Intent : " + address);
            try {
                // Tell the service to initiate a connection to the selected device
                mService.connectSensorDevice(address);
            } catch (RemoteException e) {
                Log.e(TAG, e.getLocalizedMessage());
            }
        }
    }

    // connects to existing device
    private void connectToDevice(){
        Log.i(TAG, "connecting to Neuos device");
        Intent explicit = getExplicitIntent(new Intent(NeuosSDK.NEUOS_PAIR_DEVICE));
        if (explicit == null){
            Log.i(TAG, "Cannot find Neuos pair device activity");
            return;
        }
        devicePairingBroadcastReceiver = new DevicePairingBroadcastReceiver();
        registerReceiver(devicePairingBroadcastReceiver,
                new IntentFilter(NeuosSDK.IO_NEUOS_DEVICE_PAIRING_ACTION));
        startActivity(explicit);
    }


    private Intent getExplicitIntent(Intent activityIntent){
        List<ResolveInfo> matches=getPackageManager()
                .queryIntentActivities(activityIntent , PackageManager.MATCH_ALL);
        // if we couldn't find one, return null
        if (matches.isEmpty()) {
            return null;
        }
        // there really should only be 1 match, so we get the first one.
        Intent explicit=new Intent(activityIntent);
        ActivityInfo info = matches.get(0).activityInfo;
        ComponentName cn = new ComponentName(info.applicationInfo.packageName,
                info.name);
        explicit.setComponent(cn);
        return explicit;
    }


}