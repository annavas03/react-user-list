import { StrictMode } from 'react'
import './index.css'
import App from './App.jsx'
import { GoogleOAuthProvider } from "@react-oauth/google";
import { BrowserRouter } from "react-router-dom";
import { createRoot } from 'react-dom/client';

createRoot(document.getElementById('root')).render(
    <StrictMode>
        <GoogleOAuthProvider clientId="446631909658-holrtc2ekhio529jo7h3tkujbh4c53t7.apps.googleusercontent.com">
            <BrowserRouter>
                <App />
            </BrowserRouter>
        </GoogleOAuthProvider>
    </StrictMode>
);
