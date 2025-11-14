import {useState} from "react";
import {type Book, type LoginRequestDto, type RegisterRequestDto} from "@core/generated-client.ts";
import {authApi} from "@utilities/authApi.ts";
import toast from "react-hot-toast";
import {libraryApi} from "@utilities/libraryApi.ts";
import {SieveQueryBuilder} from "ts-sieve-query-builder";

export default function Auth() {
    
    const [registerForm, setRegisterForm] = useState<RegisterRequestDto>({
        email: '',
        password: ''
    })
    const [books, setBooks] = useState<Book[]>([])


    return <>
    
         <input className="input" placeholder="please write an email here to regsiter" onChange={e => setRegisterForm({...registerForm, email: e.target.value})} />
        <input className="input" placeholder="password longer than 8 chars"   onChange={e => setRegisterForm({...registerForm, password: e.target.value})} />
        
        <button className="btn btn-primary" disabled={registerForm.password.length < 8} onClick={() => {
            authApi.register(registerForm).
            then(r => {
                localStorage.setItem('jwt', r.token)
                toast.success('welcome')
            })
        }}>register me</button>
        
        
        <button className="btn"  onClick={() => {
            const q = SieveQueryBuilder.create<Book>()
                .page(1)
                .pageSize(10)
                .sortBy("title");
            libraryApi.getBooks(q.buildSieveModel()).then(r => {
                console.log(r)
                setBooks(r)
            })
        }}>click here to fetch books (requires you to register first)</button>

        {
            books && books.length > 0 && books.map(b => {
                return <div>{b.title}</div>
            })
        }
    </>
}