import React, { useState, useEffect } from 'react';
import ReactDOM from 'react-dom';
import * as signalR from '@microsoft/signalr';

const App = () => {
    const [messages, setMessages] = useState([]);
    const [message, setMessage] = useState('');
    const [user, setUser] = useState('');

    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl('/chathub')
            .build();

        connection.on('ReceiveMessage', (user, message) => {
            setMessages(messages => [...messages, { user, message }]);
        });

        connection.start().catch(err => console.error(err));

        return () => {
            connection.stop();
        };
    }, []);

    const sendMessage = () => {
        fetch('/chathub', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ user, message })
        });
        setMessage('');
    };

    return (
        <div>
            <h1>Real-Time Chat App</h1>
            <div>
                <input
                    type="text"
                    value={user}
                    onChange={(e) => setUser(e.target.value)}
                    placeholder="Enter your name"
                />
            </div>
            <div>
                <input
                    type="text"
                    value={message}
                    onChange={(e) => setMessage(e.target.value)}
                    placeholder="Enter your message"
                />
                <button onClick={sendMessage}>Send</button>
            </div>
            <div>
                <h2>Messages</h2>
                <ul>
                    {messages.map((msg, index) => (
                        <li key={index}>
                            <strong>{msg.user}:</strong> {msg.message}
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
};

ReactDOM.render(<App />, document.getElementById('root'));
